using System;
using System.Collections;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class NotificationService : MonoBehaviour
{
    public static NotificationService Instance { get; private set; }

    // ── Android: canales (iOS no usa canales) ────────────────────────────────
    private const string CHANNEL_TURNOS   = "faerac_turnos";
    private const string CHANNEL_UPDATES  = "faerac_updates";
    private const string CHANNEL_NOTICIAS = "faerac_noticias";

    // ── Android: IDs numéricos ───────────────────────────────────────────────
    private const int ID_TURNO_DIA   = 1001;
    private const int ID_TURNO_30MIN = 1002;
    private const int ID_UPDATE      = 2001;
    private const int ID_NOTICIA     = 3001;

    // ── iOS: los identificadores son strings, no ints ────────────────────────
    private const string IOS_ID_TURNO_DIA   = "faerac_turno_dia";
    private const string IOS_ID_TURNO_30MIN = "faerac_turno_30min";
    private const string IOS_ID_UPDATE      = "faerac_update";
    private const string IOS_ID_NOTICIA     = "faerac_noticia";

    // Nombre de la tienda según plataforma.
    // En el build de iOS la cadena "Google Play" queda excluida en tiempo de
    // compilación, así que nunca entra al binario (guideline 2.3.10 de Apple).
#if UNITY_IOS
    private const string STORE_NAME = "App Store";
#else
    private const string STORE_NAME = "Google Play";
#endif

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        RegisterChannels();
        yield return RequestPermissionAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
        yield return RequestPermissionIOS();
        // Limpia el badge y las notificaciones ya entregadas al abrir la app.
        iOSNotificationCenter.ApplicationBadge = 0;
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
#else
        yield return null;
#endif
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  API PÚBLICA (idéntica en ambas plataformas: VersionChecker, NewsChecker
    //  y el flujo de turnos no necesitan cambiar nada)
    // ═════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Agenda dos notificaciones: una a las 8 AM del día del turno y otra 30
    /// minutos antes. Llámalo cuando el turno se confirme exitosamente.
    /// </summary>
    public void ScheduleAppointmentReminders(string profesionalNombre, DateTime turnoDateTime)
    {
        CancelAppointmentReminders();

        var fireTimeDia = turnoDateTime.Date.AddHours(8);
        var fireTime30  = turnoDateTime.AddMinutes(-30);

#if UNITY_ANDROID && !UNITY_EDITOR
        if (fireTimeDia > DateTime.Now)
        {
            SendAndroid(CHANNEL_TURNOS, ID_TURNO_DIA,
                "Recordatorio de turno",
                $"Hoy tenés turno con {profesionalNombre}",
                fireTimeDia);
        }

        if (fireTime30 > DateTime.Now)
        {
            SendAndroid(CHANNEL_TURNOS, ID_TURNO_30MIN,
                "Tu turno es pronto",
                $"En 30 minutos tenés turno con {profesionalNombre}",
                fireTime30);
        }

#elif UNITY_IOS && !UNITY_EDITOR
        if (fireTimeDia > DateTime.Now)
        {
            ScheduleIOSAt(IOS_ID_TURNO_DIA,
                "Recordatorio de turno",
                $"Hoy tenés turno con {profesionalNombre}",
                fireTimeDia,
                threadId: "turnos");
        }

        if (fireTime30 > DateTime.Now)
        {
            ScheduleIOSAt(IOS_ID_TURNO_30MIN,
                "Tu turno es pronto",
                $"En 30 minutos tenés turno con {profesionalNombre}",
                fireTime30,
                threadId: "turnos");
        }
#endif
    }

    /// <summary>Cancela las notificaciones de turno agendadas.</summary>
    public void CancelAppointmentReminders()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidNotificationCenter.CancelNotification(ID_TURNO_DIA);
        AndroidNotificationCenter.CancelNotification(ID_TURNO_30MIN);

#elif UNITY_IOS && !UNITY_EDITOR
        iOSNotificationCenter.RemoveScheduledNotification(IOS_ID_TURNO_DIA);
        iOSNotificationCenter.RemoveScheduledNotification(IOS_ID_TURNO_30MIN);
#endif
    }

    /// <summary>
    /// Muestra una notificación indicando que hay una nueva versión disponible.
    /// Llamado desde VersionChecker cuando detecta una versión más nueva.
    /// </summary>
    public void ShowUpdateNotification(string nuevaVersion)
    {
        var titulo = "Nueva versión disponible";
        var cuerpo = $"Actualizá FAERAC a la versión {nuevaVersion} en {STORE_NAME}";

#if UNITY_ANDROID && !UNITY_EDITOR
        SendAndroid(CHANNEL_UPDATES, ID_UPDATE, titulo, cuerpo,
            DateTime.Now.AddSeconds(3), intentData: "open_store");

#elif UNITY_IOS && !UNITY_EDITOR
        ScheduleIOSIn(IOS_ID_UPDATE, titulo, cuerpo, seconds: 3,
            threadId: "updates", data: "open_store");
#endif
    }

    /// <summary>
    /// Muestra una notificación de novedad o publicidad.
    /// Llamado desde NewsChecker cuando detecta contenido nuevo en el servidor.
    /// </summary>
    public void ShowNewsNotification(string titulo, string cuerpo)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        SendAndroid(CHANNEL_NOTICIAS, ID_NOTICIA, titulo, cuerpo,
            DateTime.Now.AddSeconds(3));

#elif UNITY_IOS && !UNITY_EDITOR
        ScheduleIOSIn(IOS_ID_NOTICIA, titulo, cuerpo, seconds: 3,
            threadId: "noticias");
#endif
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  ANDROID
    // ═════════════════════════════════════════════════════════════════════════
#if UNITY_ANDROID && !UNITY_EDITOR

    private void RegisterChannels()
    {
        AndroidNotificationCenter.RegisterNotificationChannel(new AndroidNotificationChannel
        {
            Id          = CHANNEL_TURNOS,
            Name        = "Recordatorios de Turno",
            Importance  = Importance.High,
            Description = "Aviso el día del turno y 30 minutos antes",
            EnableVibration = true,
            CanShowBadge    = true,
        });

        AndroidNotificationCenter.RegisterNotificationChannel(new AndroidNotificationChannel
        {
            Id          = CHANNEL_UPDATES,
            Name        = "Actualizaciones",
            Importance  = Importance.Default,
            Description = $"Nueva versión disponible en {STORE_NAME}",
            EnableVibration = false,
            CanShowBadge    = false,
        });

        AndroidNotificationCenter.RegisterNotificationChannel(new AndroidNotificationChannel
        {
            Id          = CHANNEL_NOTICIAS,
            Name        = "Novedades",
            Importance  = Importance.Default,
            Description = "Noticias y promociones de FAERAC",
            EnableVibration = false,
            CanShowBadge    = true,
        });
    }

    private IEnumerator RequestPermissionAndroid()
    {
        if (AndroidNotificationCenter.UserPermissionToPost != PermissionStatus.Allowed)
        {
            // PermissionRequest NO es un YieldInstruction: hay que hacer polling.
            var req = new PermissionRequest();
            while (req.Status == PermissionStatus.RequestPending)
                yield return null;
        }
    }

    private void SendAndroid(string channel, int id, string titulo, string cuerpo,
                             DateTime fireTime, string intentData = null)
    {
        AndroidNotificationCenter.SendNotificationWithExplicitID(new AndroidNotification
        {
            Title      = titulo,
            Text       = cuerpo,
            SmallIcon  = "ic_notification",
            LargeIcon  = "ic_launcher",
            FireTime   = fireTime,
            Style      = NotificationStyle.BigTextStyle,
            IntentData = intentData,
        }, channel, id);
    }

#endif

    // ═════════════════════════════════════════════════════════════════════════
    //  iOS
    // ═════════════════════════════════════════════════════════════════════════
#if UNITY_IOS && !UNITY_EDITOR

    private IEnumerator RequestPermissionIOS()
    {
        // registerForRemoteNotifications: false → solo notificaciones locales,
        // así no hace falta la capability de Push Notifications ni un servidor APNs.
        var options = AuthorizationOption.Alert
                    | AuthorizationOption.Badge
                    | AuthorizationOption.Sound;

        using (var req = new AuthorizationRequest(options, false))
        {
            while (!req.IsFinished)
                yield return null;

            if (!req.Granted)
                Debug.Log("[NotificationService] El usuario rechazó las notificaciones.");
        }
    }

    /// <summary>Agenda una notificación iOS para una fecha/hora concreta.</summary>
    private void ScheduleIOSAt(string id, string titulo, string cuerpo,
                               DateTime fireTime, string threadId = null,
                               string data = null)
    {
        var trigger = new iOSNotificationCalendarTrigger
        {
            Year    = fireTime.Year,
            Month   = fireTime.Month,
            Day     = fireTime.Day,
            Hour    = fireTime.Hour,
            Minute  = fireTime.Minute,
            Second  = fireTime.Second,
            Repeats = false,
        };

        ScheduleIOS(id, titulo, cuerpo, trigger, threadId, data);
    }

    /// <summary>Agenda una notificación iOS dentro de N segundos.</summary>
    private void ScheduleIOSIn(string id, string titulo, string cuerpo,
                               int seconds, string threadId = null,
                               string data = null)
    {
        var trigger = new iOSNotificationTimeIntervalTrigger
        {
            TimeInterval = TimeSpan.FromSeconds(Mathf.Max(1, seconds)),
            Repeats      = false,
        };

        ScheduleIOS(id, titulo, cuerpo, trigger, threadId, data);
    }

    private void ScheduleIOS(string id, string titulo, string cuerpo,
                             iOSNotificationTrigger trigger,
                             string threadId, string data)
    {
        var notification = new iOSNotification
        {
            Identifier = id,
            Title      = titulo,
            Body       = cuerpo,
            Data       = data ?? string.Empty,
            // iOS no muestra notificaciones con la app en primer plano salvo
            // que se lo pidas explícitamente:
            ShowInForeground = true,
            ForegroundPresentationOption = PresentationOption.Alert
                                         | PresentationOption.Sound
                                         | PresentationOption.Badge,
            ThreadIdentifier = threadId ?? "faerac",
            Trigger = trigger,
        };

        // Reemplaza cualquier notificación previa con el mismo Identifier.
        iOSNotificationCenter.ScheduleNotification(notification);
    }

#endif
}