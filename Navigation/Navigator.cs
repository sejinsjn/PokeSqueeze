using MVVMCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Navigation
{
    public class Navigator
    {
        private static Navigator instance = new Navigator();

        private const string VIEW_MODEL = "ViewModel";

        private bool enableUIExtensibility = false;

        private NavigationNode rootNavigationNode = new NavigationNode();

        private int currentLevel = 0;
        private string currentCaption = string.Empty;
        private List<int> currentNavigationIndicesOfLevels = new List<int>() { 0 };
        public event EventHandler Navigated;
        public event EventHandler LastNavigation;

        public bool Enabled { get; set; } = true;

        private Navigator()
        {
        }

        public static Navigator Instance
        {
            get
            {
                return instance;
            }
        }
        
        public NavigationNode NavigationService
        {
            get
            {
                return rootNavigationNode;
            }

            set
            {
                rootNavigationNode = value;
            }
        }

        public bool EnableUIExtensibility
        {
            set
            {
                enableUIExtensibility = value;
            }
        }

        public int CurrentLevel
        {
            set
            {
                currentLevel = value;
            }
        }


        public string NavigationPath
        {
            get
            {
                if (rootNavigationNode.NavigationService.Content != null)
                {
                    return rootNavigationNode.NavigationService.Content.GetType().Name;
                }
                return string.Empty;
            }
        }

        public List<int> CurrentNavigationIndicesOfLevels
        {
            get
            {
                return currentNavigationIndicesOfLevels;
            }

            set
            {
                currentNavigationIndicesOfLevels = value;
            }
        }

        public void NavigateToWindow(Type typeOfWindow, string assemblyName)
        {
            NavigateToWindow(typeOfWindow, null, assemblyName);
        }

        public void NavigateToWindow(Type typeOfWindow, object data, string assemblyName)
        {
            if (!Enabled)
            {
                return;
            }

            try
            {
                object window = null;

                string typeName = typeOfWindow.FullName;
                if (typeName.StartsWith(assemblyName))
                {
                    typeName = typeName.Substring(assemblyName.Length + 1);
                }

                if (window == null)
                {
                    window = Activator.CreateInstance(typeOfWindow);
                }


                Type type = Type.GetType(typeName + VIEW_MODEL + ", " + assemblyName);
                if (type == null)
                {
                    type = FindType(assemblyName + ".ViewModels." + typeName + VIEW_MODEL);
                }

                object viewModel = Activator.CreateInstance(type) as ViewModelBase;

                if (window is Window)
                {
                    if (viewModel is ViewModelBase)
                    {
                        (viewModel as ViewModelBase).Initialize(data);
                        (window as Window).DataContext = viewModel;

                    }

                    if (window is INavigatable)
                    {
                        (window as INavigatable).NavigationServiceContentList[0].Navigated -= navigationService_Navigated;
                        (window as INavigatable).NavigationServiceContentList[0].Navigated += navigationService_Navigated;
                        rootNavigationNode.GetNavigationNode(currentLevel, currentNavigationIndicesOfLevels).NavigationService = (window as INavigatable).NavigationServiceContentList[0];
                        rootNavigationNode.GetNavigationNode(currentLevel, currentNavigationIndicesOfLevels).SubNavigationNodeList =
                            new List<NavigationNode>();
                    }

                (window as Window).Show();
                    (window as Window).Activate();

                    if (viewModel is ViewModelBase)
                    {
                        (viewModel as ViewModelBase).OnAfterActivated();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Refresh()
        {
            GetNavigationService().Refresh();
        }

        private NavigationService GetNavigationService()
        {
            return rootNavigationNode.GetNavigationService(currentLevel, currentNavigationIndicesOfLevels);
        }

        public void GoBack()
        {
            if (!Enabled)
            {
                return;
            }

            NavigationService navigationService =
                rootNavigationNode.GetNavigationService(currentLevel, currentNavigationIndicesOfLevels);
            if (navigationService != null)
            {
                if (navigationService.CanGoBack)
                {
                    navigationService.GoBack();
                }
            }
        }

        private Type FindType(string fullName)
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName.Equals(fullName));
        }

        public void NavigateToPage(string typeNameOfPage, object data, string assemblyName)
        {
            if(!Enabled)
            {
                return;
            }

            try
            {
                Type type = Type.GetType(typeNameOfPage + ", " + assemblyName);
                if (type == null)
                {
                    type = FindType(typeNameOfPage);
                }
                object page = null;
                if (type != null)
                {
                    page = Activator.CreateInstance(FindType(typeNameOfPage));
                }
                
                string typeNameOfViewModel = typeNameOfPage.Replace("Pages", "ViewModels");

                switch (typeNameOfPage)
                {
                    case "ConfigurationUI.Pages.ConfigurationPage":
                        typeNameOfViewModel = "Configurator.Core.ViewModels.ConfigurationViewModel";
                        assemblyName = "Configurator.Core";
                        break;
                    case "EnergyAndMediaConsumption.Tool.Pages.TCOTable":
                        typeNameOfViewModel = "EnergyAndMediaConsumption.Tool.ViewModels.MainViewModel";
                        break;
                    default:
                        typeNameOfViewModel = typeNameOfViewModel.Remove(typeNameOfViewModel.Length - 4) + VIEW_MODEL;
                        break;
                }

                Type viewModelType = Type.GetType(typeNameOfViewModel + ", " + assemblyName);
                object viewModel = null;
                if (viewModelType == null)
                {
                    viewModelType = FindType(typeNameOfViewModel);
                }
                if (viewModelType != null)
                {
                    viewModel = Activator.CreateInstance(viewModelType) as ViewModelBase;
                }

                if (page is Page && rootNavigationNode != null)
                {
                    NavigationService navigationService = GetNavigationService();

                    if ((navigationService != null) && (navigationService.Content is Page))
                    {
                        if ((navigationService.Content as Page).DataContext is ViewModelBase)
                        {
                            ((navigationService.Content as Page).DataContext as ViewModelBase).OnBeforeClose();
                        }
                    }

                    if (page is INavigatable)
                    {
                        List<NavigationNode> subNavigationNodeList
                            = (page as INavigatable).NavigationServiceContentList.Select(x => new NavigationNode(currentLevel + 1, x)).ToList();

                        foreach (NavigationNode navigationNode in subNavigationNodeList)
                        {
                            navigationNode.NavigationService.Navigated -= navigationService_Navigated;
                            navigationNode.NavigationService.Navigated += navigationService_Navigated;
                        }
                        rootNavigationNode.GetNavigationNode(currentLevel, currentNavigationIndicesOfLevels).SubNavigationNodeList =
                            subNavigationNodeList;
                    }
                    if (viewModel is ViewModelBase)
                    {
                        (viewModel as ViewModelBase).Initialize(data);
                        (page as Page).DataContext = viewModel;
                    }

                    if (navigationService != null)
                    {
                        navigationService.Navigate(page);
                    }

                    if (viewModel is ViewModelBase)
                    {
                        (viewModel as ViewModelBase).OnAfterActivated();
                    }

                    LastNavigation?.Invoke(null, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void NavigateToPage(Type typeOfPage, object data, string assemblyName)
        {
            NavigateToPage(typeOfPage.ToString(), data, assemblyName);
        }

        private void navigationService_Navigated(object sender, NavigationEventArgs e)
        {
            Navigated?.Invoke(null, EventArgs.Empty);
        }
    }
}
