using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace UP_Denisov_Coffeeshop
{
    public partial class Entities : DbContext
    {
        private static Entities _context;

        public static Entities GetContext()
        {
            if (_context == null)
                _context = new Entities();
            return _context;
        }

        public void Refresh()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }

        public bool TrySave(string successMessage = null)
        {
            try
            {
                SaveChanges();
                if (!string.IsNullOrEmpty(successMessage))
                {
                    MessageBox.Show(successMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных в БД:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Refresh();
                return false;
            }
        }
    }

    public partial class Payment
    {
        public decimal Total => Num * Price;
    }
}