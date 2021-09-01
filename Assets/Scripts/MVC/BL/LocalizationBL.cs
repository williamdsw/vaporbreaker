using System.Collections.Generic;
using MVC.DAO;
using MVC.Models;

namespace MVC.BL
{
    public class LocalizationBL
    {
        private LocalizationDAO localizationDAO = new LocalizationDAO();

        public Localization GetByLanguage(string language) => localizationDAO.GetByLanguage(language);

        public List<Localization> GetLanguages() => localizationDAO.GetLanguages();
    }
}