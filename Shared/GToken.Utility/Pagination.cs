using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Platform.Utility.Helpers.Extensions
{
    public class Pagination
    {
        public int page { get; set; }
        public int per_page { get; set; }
        public int total_count { get; set; }

        public Pagination(int page, int per_page, int total_count)
        {
            this.page = page;
            this.per_page = per_page;
            this.total_count = total_count;
        }
        public int pages { get { return int.Parse(Math.Ceiling(this.total_count / float.Parse(this.per_page.ToString())).ToString()); } }


        public bool has_prev { get { return page > 1; } }
        public bool has_next { get { return page < pages; } }

        public List<int> iter_pages(int left_edge = 2, int left_current = 2,
                    int right_current = 5, int right_edge = 2)
        {
            List<int> iter_pages = new List<int>();
            int last = 0;
            foreach (var num in Enumerable.Range(1, pages))
            {
                if (num <= left_edge || (num > page - left_current - 1 && num < page + right_current) || num > pages - right_edge)
                {
                    if (last + 1 != num)
                    {
                        iter_pages.Add(0);
                    }
                    iter_pages.Add(num);
                    last = num;
                }
            }
            return iter_pages;
        }
        //def iter_pages(self, left_edge=2, left_current=2,
        //               right_current=5, right_edge=2):
        //    last = 0
        //    for num in xrange(1, self.pages + 1):
        //        if num <= left_edge or \
        //           (num > self.page - left_current - 1 and \
        //            num < self.page + right_current) or \
        //           num > self.pages - right_edge:
        //            if last + 1 != num:
        //                yield None
        //            yield num
        //            last = num

    }
}