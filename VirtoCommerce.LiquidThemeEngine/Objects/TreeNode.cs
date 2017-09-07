using DotLiquid;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class TreeNode : Drop
    {
        public TreeNode()
        {
            Children = new List<TreeNode>();
        }

        public string Path { get; set; }

        public int Level { get; set; }

        public string Title { get; set; }

        public int? Priority { get; set; }

        public ICollection<TreeNode> Children { get; set; }

        public string ParentPath { get; set; }

        public TreeNode Parent { get; set; }

        public ICollection<TreeNode> Parents
        {
            get
            {
                var parents = new TreeNode[] { };

                if (Parent != null)
                {
                    parents = Parent.Parents.Concat(new[] { Parent }).ToArray();
                }

                return parents;
            }
        }

        public ICollection<TreeNode> AllChildren { get; set; }
    }
}