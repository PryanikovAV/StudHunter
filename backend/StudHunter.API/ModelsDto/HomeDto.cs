namespace StudHunter.API.ModelsDto;

public record CategoryCardDto(
    string Title,           // Например: "Стажировки" или "IT, интернет, связь"
    int Count,              // Количество вакансий
    string FilterKey,       // Ключ для Vue Router (например: "vacancyType" или "specializationId")
    string FilterValue      // Значение фильтра (например: "Internship" или "guid-сферы")
);

public record HomePageDto(
    List<CategoryCardDto> PopularCategories,
    List<VacancySearchDto> LatestVacancies
);