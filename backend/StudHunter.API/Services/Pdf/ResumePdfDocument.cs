using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StudHunter.API.Infrastructure;
using StudHunter.API.ModelsDto;

namespace StudHunter.API.Services.Pdf;

public class ResumePdfDocument(ResumeSearchDto resume) : IDocument
{
    private readonly ResumeSearchDto _resume = resume;

    private readonly string DarkText = "#111827";
    private readonly string GrayText = "#6B7280";
    private readonly string GrayBorder = "#E5E7EB";
    private readonly string SusuBlue = "#005AAA";
    private readonly string BackgroundPage = "#F8FAFC";

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);

                page.DefaultTextStyle(x => x
                    .FontSize(11)
                    .FontFamily(Fonts.Arial)
                    .FontColor(DarkText));

                page.Content().Element(ComposeContent);
            });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Spacing(20);

            // Блок 1: Шапка
            column.Item().Element(ComposeHeader);

            // Блок 2: Контакты
            if (!string.IsNullOrEmpty(_resume.Email) || !string.IsNullOrEmpty(_resume.Phone))
                column.Item().Element(ComposeContacts);

            // Блок 3: Образование
            if (!string.IsNullOrEmpty(_resume.UniversityName) || !string.IsNullOrEmpty(_resume.FacultyName))
                column.Item().Element(ComposeEducation);

            // Блок 3.2: Дисциплины
            if (_resume.CompletedCourses != null && _resume.CompletedCourses.Any())
                column.Item().Element(c => ComposeTagsSection(c, "Пройденные дисциплины", _resume.CompletedCourses));

            // Блок 4: Навыки
            if (_resume.Skills != null && _resume.Skills.Any())
                column.Item().Element(c => ComposeTagsSection(c, "Ключевые навыки", _resume.Skills));

            // Блок 5: О себе
            if (!string.IsNullOrEmpty(_resume.Description))
                column.Item().Element(ComposeAbout);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem(3).Column(column =>
            {
                column.Item().Text(_resume.Title).FontSize(24).SemiBold().FontColor(DarkText);
                column.Item().PaddingTop(4).Text(_resume.FullName).FontSize(16).SemiBold();
                column.Item().PaddingTop(6).Inlined(inlined =>
                {
                    inlined.Spacing(6);

                    void AddSeparator() => inlined.Item().Text("•").FontColor(GrayText);

                    var ageStr = UserDisplayHelper.GetAgeString(_resume.BirthDate);
                    if (ageStr != null)
                    {
                        inlined.Item().Text(ageStr).FontColor(DarkText);
                        AddSeparator();
                    }

                    if (!string.IsNullOrEmpty(_resume.Gender))
                    {
                        inlined.Item().Text(_resume.Gender == "Male" ? "Мужской" : "Женский").FontColor(DarkText);
                        AddSeparator();
                    }

                    if (!string.IsNullOrEmpty(_resume.CityName))
                    {
                        inlined.Item().Text($"г. {_resume.CityName}").FontColor(DarkText);
                        AddSeparator();
                    }

                    string citizenship = _resume.IsForeign ? "Иностранный гражданин" : "Гражданин РФ";
                    inlined.Item().Text(citizenship).FontColor(DarkText);
                });

                if (_resume.UpdatedAt != default)
                {
                    column.Item().PaddingTop(4).Text($"Обновлено: {_resume.UpdatedAt:dd MMMM yyyy} г.")
                        .FontSize(10).FontColor(GrayText);
                }
            });

            row.ConstantItem(100).Column(column =>
            {
                column.Item()
                    .Width(80)
                    .Height(80)
                    .Background(BackgroundPage)
                    .Border(1)
                    .BorderColor(GrayBorder)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text("ФОТО")
                    .FontColor(GrayText);

                if (!string.IsNullOrEmpty(_resume.Status))
                {
                    column.Item().PaddingTop(8)
                        .Width(80)
                        .AlignCenter()
                        .Text(MapStatus(_resume.Status))
                        .FontSize(11)
                        .SemiBold()
                        .FontColor(SusuBlue);
                }
            });
        });
    }

    private void ComposeContacts(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => ComposeSectionHeader(c, "Контакты"));

            col.Item().PaddingTop(8).Column(column =>
            {
                if (!string.IsNullOrEmpty(_resume.Email))
                    ComposeKeyValue(column, "Email:", _resume.Email);

                if (!string.IsNullOrEmpty(_resume.Phone))
                    ComposeKeyValue(column, "Телефон:", _resume.Phone);
            });
        });
    }

    private void ComposeEducation(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => ComposeSectionHeader(c, "Образование"));

            col.Item().PaddingTop(8).Column(column =>
            {
                if (!string.IsNullOrEmpty(_resume.UniversityName)) ComposeKeyValue(column, "ВУЗ:", _resume.UniversityName);
                if (!string.IsNullOrEmpty(_resume.FacultyName)) ComposeKeyValue(column, "Факультет:", _resume.FacultyName);
                if (!string.IsNullOrEmpty(_resume.DepartmentName)) ComposeKeyValue(column, "Кафедра:", _resume.DepartmentName);
                if (!string.IsNullOrEmpty(_resume.StudyDirectionName)) ComposeKeyValue(column, "Направление:", _resume.StudyDirectionName);

                if (_resume.CourseNumber.HasValue)
                {
                    var form = _resume.StudyForm switch
                    {
                        "FullTime" => "Очная",
                        "PartTime" => "Очно-заочная",
                        "Correspondence" => "Заочная",
                        _ => _resume.StudyForm
                    };
                    ComposeKeyValue(column, "Курс и форма:", $"{_resume.CourseNumber} курс, {form}");
                }
            });
        });
    }

    private void ComposeAbout(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => ComposeSectionHeader(c, "О себе"));
            col.Item().PaddingTop(8).Text(_resume.Description).LineHeight(1.4f);
        });
    }

    private void ComposeSectionHeader(IContainer container, string title)
    {
        container.BorderBottom(1).BorderColor(GrayBorder).PaddingBottom(4).Text(title).FontSize(14).SemiBold();
    }

    private void ComposeKeyValue(ColumnDescriptor column, string label, string value)
    {
        column.Item().PaddingBottom(4).Row(row =>
        {
            row.ConstantItem(150).Text(label).SemiBold().FontColor(GrayText);
            row.RelativeItem().Text(value);
        });
    }

    private void ComposeTagsSection(IContainer container, string title, IEnumerable<string> items)
    {
        container.Column(col =>
        {
            col.Item().Element(c => ComposeSectionHeader(c, title));

            col.Item().PaddingTop(8).Inlined(inlined =>
            {
                inlined.Spacing(6);
                foreach (var item in items)
                {
                    inlined.Item().Background(BackgroundPage).Border(1).BorderColor(GrayBorder)
                        .PaddingHorizontal(8).PaddingVertical(3).Text(item).FontSize(10).FontColor(DarkText);
                }
            });
        });
    }

    private string MapStatus(string status) => status switch
    {
        "Studying" => "Учусь",
        "SeekingInternship" => "Ищу стажировку",
        "SeekingJob" => "Ищу работу",
        "Interning" => "На стажировке",
        "Working" => "Работаю",
        _ => status
    };
}