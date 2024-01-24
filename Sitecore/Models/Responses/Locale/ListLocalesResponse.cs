using Sitecore.Models.Entities;

namespace Sitecore.Models.Responses.Locale;

public record ListLocalesResponse(IEnumerable<LocaleEntity> Locales);