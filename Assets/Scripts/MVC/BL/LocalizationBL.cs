using MVC.DAO;
using MVC.Models;
using System.Collections.Generic;

namespace MVC.BL
{
    /// <summary>
    /// Business Layer for Localization
    /// </summary>
    public class LocalizationBL
    {
        private LocalizationDAO localizationDAO = new LocalizationDAO();

        /// <summary>
        /// Get a localization by language
        /// </summary>
        /// <param name="language"> Desired language </param>
        /// <returns> Localization instance </returns>
        public Localization GetByLanguage(string language) => localizationDAO.GetByLanguage(language);

        /// <summary>
        /// List all languages
        /// </summary>
        /// <returns> List of Localization instances </returns>
        public List<Localization> ListAll() => localizationDAO.ListAll();
    }
}