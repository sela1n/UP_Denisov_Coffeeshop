using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class AddCategoryPage : Page
    {
        private readonly Category _category;

        public AddCategoryPage(Category category)
        {
            InitializeComponent();
            _category = category ?? new Category();
            InputHelper.Letters(TbName);

            if (_category.ID == 0)
                Title = "Добавление категории";
            else
            {
                Title = "Редактирование категории";
                TbName.Text = _category.Name;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = TbName.Text.Trim();
            var db = Entities.GetContext();

            var errors = new StringBuilder();
            if (!Regex.IsMatch(name, @"^[А-Яа-яЁёA-Za-z]+([ \-][А-Яа-яЁёA-Za-z]+)*$") || name.Length < 2)
                errors.AppendLine("Название: от 2 символов, только буквы, пробелы и дефис.");
            else if (db.Category.Any(c => c.Name == name && c.ID != _category.ID))
                errors.AppendLine("Такая категория уже существует!");

            if (errors.Length > 0)
            {
                MessageHelper.Warning(errors.ToString());
                return;
            }

            _category.Name = name;
            if (_category.ID == 0)
                db.Category.Add(_category);

            string error;
            if (!db.TrySave(out error))
            {
                MessageHelper.Error(error);
                return;
            }
            MessageHelper.Info("Данные успешно сохранены!");
            NavigationService.GoBack();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TbName.Clear();
        }
    }
}
