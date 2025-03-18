using System;
using System.Collections.Generic;
using System.Linq;
using eLearnApps.ViewModel.RPT;

namespace eLearnApps.ViewModel.Valence
{
    public class GradeScheme
    {
        public string Route { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public List<GradeSchemeRange> Ranges { get; set; }

        public GradeSchemeViewModel ToViewModel()
        {
            var tmp = new GradeSchemeViewModel()
            {
                GradeSchemeId = this.Id,
                GradeSchemeUrl = this.Route,
                Name = this.Name,
                Schemes = new List<GradeSchemeRangeViewModel>()
            };

            // must be ordered from low to highest
            this.Ranges = this.Ranges.OrderBy(r => r.PercentStart).ToList();
            for (int i = 0; i < this.Ranges.Count(); i++)
            {
                if (i == this.Ranges.Count() - 1)
                {
                    tmp.Schemes.Insert(0, new GradeSchemeRangeViewModel()
                    {
                        Label = $">= {this.Ranges[i].PercentStart}%",
                        MinPoint = this.Ranges[i].PercentStart,
                        MaxPoint = double.MaxValue,
                        Grade = this.Ranges[i].Symbol,
                        Colour = this.Ranges[i].Colour,
                        Order = this.Ranges.Count() - i
                    });
                }
                else
                {
                    tmp.Schemes.Insert(0, new GradeSchemeRangeViewModel()
                    {
                        Label = $">= {this.Ranges[i].PercentStart}% and < {this.Ranges[i + 1].PercentStart}%",
                        MinPoint = this.Ranges[i].PercentStart,
                        MaxPoint = this.Ranges[i + 1].PercentStart,
                        Grade = this.Ranges[i].Symbol,
                        Colour = this.Ranges[i].Colour,
                        Order = this.Ranges.Count() - i
                    });
                }
            }

            return tmp;
        }
    }

    public class GradeSchemeRange
    {
        public double PercentStart { get; set; }
        public string Symbol { get; set; }
        public double AssignedValue { get; set; }
        public string Colour { get; set; }
    }
}
