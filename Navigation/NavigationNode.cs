using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Navigation
{
    public class NavigationNode
    {
        private int level = 0;
        private string currentCaption = null;
        private NavigationService navigationService = null;
        private List<NavigationNode> subNavigationNodeList = new List<NavigationNode>();
        
        public NavigationNode()
        {
        }

        public NavigationNode(int level = 0, string currentCaption = "Caption")
        {
            this.level = level;
            this.currentCaption = currentCaption;
        }

        public NavigationNode(int level, NavigationService navigationService)
        {
            this.level = level;
            this.navigationService = navigationService;
        }

        public NavigationService NavigationService
        {
            get
            {
                return navigationService;
            }

            set
            {
                navigationService = value;
            }
        }
        
        public List<NavigationNode> SubNavigationNodeList
        {
            get
            {
                return subNavigationNodeList;
            }

            set
            {
                subNavigationNodeList = value;
            }
        }

        public NavigationService GetNavigationService(int level, List<int> indicesOfLevels)
        {
            if (this.level == level)
            {
                return navigationService;
            }
            if(subNavigationNodeList != null) 
            {
                if (subNavigationNodeList.Count > indicesOfLevels[0])
                {
                    return subNavigationNodeList[indicesOfLevels[0]].GetNavigationService(level, indicesOfLevels.Skip(1).ToList());
                }
            }
            return null;
        }

        internal NavigationNode GetNavigationNode(int level, List<int> indicesOfLevels)
        {
            if (this.level == level)
            {
                return this;
            }
            if (subNavigationNodeList != null)
            {
                if (subNavigationNodeList.Count > indicesOfLevels[0])
                {
                    return subNavigationNodeList[indicesOfLevels[0]].GetNavigationNode(level, indicesOfLevels.Skip(1).ToList());
                }
                else
                {
                    //TODO check
                    return subNavigationNodeList[0].GetNavigationNode(level, indicesOfLevels.Skip(1).ToList());
                }
            }
            return null;
        }

        

        /*
        public void AddNavigationNode(NavigationNode subNavigationNode, int level)
        {
            if(this.level == level)
            {
                subNavigationNode.level = level + 1;
                this.subNavigationNode = subNavigationNode;
            }
            else if(subNavigationNode != null)
            {
                AddNavigationNode(subNavigationNode, level);
            }
        }*/
    }
}
