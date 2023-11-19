using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Pigging
{
    public static class Pager
    {
        public static BasePaging Build(int pageId, int allEntityCount, int Take, int countForShowAfterAndBefore)
        {
            var pageCount = Convert.ToInt32(Math.Ceiling(allEntityCount / (double)Take));

            return new BasePaging
            {
                PageId = pageId,
                AllEntityCount = allEntityCount,
                CountForShowAfterAndBefore = countForShowAfterAndBefore,
                SkipEntity = (pageId - 1) * Take,
                TakeEntity = Take,
                StartPage = pageId - countForShowAfterAndBefore <= 0 ? 1 : pageId - countForShowAfterAndBefore,
                EndPage = pageId + countForShowAfterAndBefore > pageCount ? pageCount : pageId + countForShowAfterAndBefore,
                PageCount = pageCount
            };
        }
    }
}
