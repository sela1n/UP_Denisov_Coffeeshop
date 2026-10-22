using System;
using System.Data.Entity;
using System.Linq;

namespace UP_Denisov_Coffeeshop
{
    public partial class Entities
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
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                try { entry.Reload(); }
                catch { entry.State = EntityState.Detached; } 
            }
        }

        public bool TrySave(out string error)
        {
            try
            {
                SaveChanges();
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = "Не удалось сохранить данные: " + ex.GetBaseException().Message;
                foreach (var entry in ChangeTracker.Entries().ToList())
                {
                    if (entry.State == EntityState.Added)
                        entry.State = EntityState.Detached;
                    else if (entry.State != EntityState.Unchanged)
                    {
                        try { entry.Reload(); }
                        catch { entry.State = EntityState.Detached; }
                    }
                }
                return false;
            }
        }
    }

    public partial class Payment
    {
        public decimal Total
        {
            get { return Num * Price; }
        }
    }
}
