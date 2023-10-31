using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Models.BaseEntities
{
    public class BaseEntity
    {
        #region properties

        [Key]
        public long Id { get; set; }

        public bool IsDelete { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        #endregion
    }

}
