using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveUp.Models
{
    /// <summary>
    /// The Model for the storage settings
    /// </summary>
    public class StorageSettings
    {
        public string? Username { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }


        public bool IsSome()
        {
            if (AccessToken != null && RefreshToken != null)
            {
                return true;
            }
            return false;
        }
    }
}
