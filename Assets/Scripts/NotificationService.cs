using System;
using System.Collections;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class NotificationService : MonoBehaviour
{
    public static NotificationService Instance { get; private set; }

    private const string CHANNEL_TURNOS   = "faerac_turnos";
    private const string CHANNEL_UPDATES  = "faerac_updates";
    private const string CHANNEL_NOTICIAS = "faerac_noticias";

    private const int ID_TURNO_DIA   = 1001;
    private const int ID_TURNO_30MIN = 1002;
    private const int ID_UPDATE      = 2001;
    private const int ID_NOTICIA     = 3001;

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
        yield return RequestPermission();
#else
        yield return null;
#endif
    }

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
            Description = "Nueva versión disponible en Google Play",
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

    private IEnumerator RequestPermission()
    {
        if (AndroidNotificationCenter.UserPermissionToPost != PermissionStatus.Allowed)
        {
            var req = new PermissionRequest();
            yield return req;
        }
    }

    // ── Turnos ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Agenda dos notificaciones: una a las 8 AM del día del turno y otra 30
    /// minutos antes. Llámalo cuando el turno se confirme exitosamente.
    /// </summary>
    public void ScheduleAppointmentReminders(string profesionalNombre, DateTime turnoDateTime)
    {
        CancelAppointmentReminders();

        var fireTimeDia = turnoDateTime.Date.AddHours(8);
        if (fireTimeDia > DateTime.Now)
        {
            AndroidNotificationCenter.SendNotificationWithExplicitID(new AndroidNotification
            {
                Title  = "Recordatorio de turno",
                Text   = $"Hoy tenés turno con {profesionalNombre}",
                SmallIcon  = "ic_notification",
                LargeIcon  = "ic_launcher",
                FireTime   = fireTimeDia,
                Style      = NotificationStyle.BigTextStyle,
            }, CHANNEL_TURNOS, ID_TURNO_DIA);
        }

        var fireTime30 = turnoDateTime.AddMinutes(-30);
        if (fireTime30 > DateTime.Now)
        {
            AndroidNotificationCenter.SendNotificationWithExplicitID(new AndroidNotification
            {
                Title  = "Tu turno es pronto",
                Text   = $"En 30 minutos tenés turno con {profesionalNombre}",
                SmallIcon  = "ic_notification",
                LargeIcon  = "ic_launcher",
                FireTime   = fireTime30,
                Style      = NotificationStyle.BigTextStyle,
            }, CHANNEL_TURNOS, ID_TURNO_30MIN);
        }
    }

    /// <summary>Cancela las notificaciones de turno agendadas.</summary>
    public void CancelAppointmentReminders()
    {
        AndroidNotificationCenter.CancelNotification(ID_TURNO_DIA);
        AndroidNotificationCenter.CancelNotification(ID_TURNO_30MIN);
    }

    // ── Actualización ────────────────────────────────────────────────────────

    /// <summary>
    /// Muestra una notificación indicando que hay una nueva versión disponible.
    /// Llamado desde VersionChecker cuando detecta una versión más nueva.
    /// </summary>
    public void ShowUpdateNotification(string nuevaVersion)
    {
        AndroidNotificationCenter.SendNotificationWithExplicitID(new AndroidNotification
        {
            Title      = "Nueva versión disponible",
            Text       = $"Actualizá FAERAC a la versión {nuevaVersion} en Google Play",
            SmallIcon  = "ic_notification",
            LargeIcon  = "ic_launcher",
            FireTime   = DateTime.Now.AddSeconds(3),
            IntentData = "open_play_store",
        }, CHANNEL_UPDATES, ID_UPDATE);
    }

    // ── Novedades / Publicidad ───────────────────────────────────────────────

    /// <summary>
    /// Muestra una notificación de novedad o publicidad.
    /// Llamado desde NewsChecker cuando detecta contenido nuevo en el servidor.
    /// </summary>
    public void ShowNewsNotification(string titulo, string cuerpo)
    {
        AndroidNotificationCenter.SendNotificationWithExplicitID(new AndroidNotification
        {
            Title      = titulo,
            Text       = cuerpo,
            SmallIcon  = "ic_notification",
            LargeIcon  = "ic_launcher",
            FireTime   = DateTime.Now.AddSeconds(3),
            Style      = NotificationStyle.BigTextStyle,
        }, CHANNEL_NOTICIAS, ID_NOTICIA);
    }

// #else
//     // Stubs para el Editor y otras plataformas
//     public void ScheduleAppointmentReminders(string profesionalNombre, DateTime turnoDateTime) { }
//     public void CancelAppointmentReminders() { }
//     public void ShowUpdateNotification(string nuevaVersion) { }
//     public void ShowNewsNotification(string titulo, string cuerpo) { }
// #endif
#else
    // Stubs para el Editor y otras plataformas (Modificalos para verlos en consola)
    public void ScheduleAppointmentReminders(string profesionalNombre, DateTime turnoDateTime) 
    { 
        Debug.Log($"[EDITOR TEST] Se agendó turno local con {profesionalNombre} para el {turnoDateTime}");
    }
    
    public void CancelAppointmentReminders() 
    { 
        Debug.Log("[EDITOR TEST] Se cancelaron las notificaciones de turnos.");
    }
    
    public void ShowUpdateNotification(string nuevaVersion) 
    { 
        Debug.Log($"[EDITOR TEST] Trigger de Actualización disparado. Versión: {nuevaVersion}");
    }
    
    public void ShowNewsNotification(string titulo, string cuerpo) 
    { 
        Debug.Log($"[EDITOR TEST] Trigger de Novedad disparado. Título: {titulo} | Cuerpo: {cuerpo}");
    }
#endif
}