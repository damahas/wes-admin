using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;

namespace Wes.ViewModel.SystemManage
{
    public class MenuRootInfo : MenuInfo
    {
        public bool AlwaysShow
        {
            get
            {
                return this.Children != null && this.Children.Count > 0;
            }
        }
        public string Redirect { set; get; }
        public List<MenuRootInfo> Children { set; get; }
    }

    public class MenuInfo
    {
        [JsonIgnore]
        public long MenuId { set; get; }
        [JsonIgnore]
        public long ParentId { set; get; }
        public string Component { set; get; }
        public bool Hidden { set; get; }
        public MenuMetaInfo Meta { set; get; }
        public string Name { set; get; }
        public string Path { set; get; }
        [JsonIgnore]
        public int OrderNum { set; get; }
    }

    public class MenuMetaInfo
    {
        public string Title { set; get; }
        public string Icon { set; get; }
        public string Link
        {
            set { }
            get
            {
                return null;
            }
        }
        public bool NoCache { set; get; }
    }
}
