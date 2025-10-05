using System.Runtime.Serialization;

namespace VkOrdApi.Creative;

public enum VkOrdCreativeForm
{
    /// <summary>
    /// Баннер
    /// </summary>
    [EnumMember(Value = "banner")]
    Banner,

    /// <summary>
    /// Текстовый блок
    /// </summary>
    [EnumMember(Value = "text_block")]
    TextBlock,

    /// <summary>
    /// Текстово-графический блок
    /// </summary>
    [EnumMember(Value = "text_graphic_block")]
    TextGraphicBlock,

    /// <summary>
    /// Аудиозапись
    /// </summary>
    [EnumMember(Value = "audio")]
    Audio,

    /// <summary>
    /// Видеоролик
    /// </summary>
    [EnumMember(Value = "video")]
    Video,

    /// <summary>
    /// Аудиотрансляция в прямом эфире
    /// </summary>
    [EnumMember(Value = "live_audio")]
    LiveAudio,

    /// <summary>
    /// Видеотрансляция в прямом эфире
    /// </summary>
    [EnumMember(Value = "live_video")]
    LiveVideo,

    /// <summary>
    /// Текстовый блок с видео
    /// </summary>
    [EnumMember(Value = "text_video_block")]
    TextVideoBlock,

    /// <summary>
    /// Текстово-графический блок с видео
    /// </summary>
    [EnumMember(Value = "text_graphic_video_block")]
    TextGraphicVideoBlock,

    /// <summary>
    /// Текстовый блок с аудио
    /// </summary>
    [EnumMember(Value = "text_audio_block")]
    TextAudioBlock,

    /// <summary>
    /// Текстово-графический блок с аудио
    /// </summary>
    [EnumMember(Value = "text_graphic_audio_block")]
    TextGraphicAudioBlock,

    /// <summary>
    /// Текстовый блок с аудио и видео
    /// </summary>
    [EnumMember(Value = "text_audio_video_block")]
    TextAudioVideoBlock,

    /// <summary>
    /// Текстово-графический блок с аудио и видео
    /// </summary>
    [EnumMember(Value = "text_graphic_audio_video_block")]
    TextGraphicAudioVideoBlock,

    /// <summary>
    /// HTML5-баннер
    /// </summary>
    [EnumMember(Value = "banner_html5")]
    BannerHtml5
}
