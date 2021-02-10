using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Flex.Database.UserInterface.ViewModels
{
    public class DB_ParentElementsViewModel : INotifyPropertyChanged
    {
        internal DB_ParentElementsViewModel()
        {
            LoadVisibleParentsAsync();
            FlexData.Context.SaveCompleted += DataContext_SaveCompleted;
        }

        private void DataContext_SaveCompleted(FlexibleRealization.IElementTreeNode saved)
        {
            LoadedParentViewModels.AddRange(ViewModelsFor(NewParentElements));
            OnPropertyChanged("VisibleParents");
        }

        //private void DataContext_ParentChanged(int parentID)
        //{
        //    OnPropertyChanged("VisibleParents");
        //}

        internal void Detach()
        {
            FlexData.Context.SaveCompleted -= DataContext_SaveCompleted;
        }
        private DB_ParentElementViewModel ViewModelFor(DB_Parent parentElement) => new DB_ParentElementViewModel(parentElement);

        private IEnumerable<DB_ParentElementViewModel> ViewModelsFor(IEnumerable<DB_Parent> parentElements) => parentElements.Select(parentElement => ViewModelFor(parentElement));

        private IEnumerable<DB_Parent> NewParentElements => FlexData.Context.DB_Parents.ToList().Where(dbParent => !LoadedParentViewModels.Any(viewModel => viewModel.DB_Parent == dbParent));


        private List<DB_ParentElementViewModel> LoadedParentViewModels = new List<DB_ParentElementViewModel>();

        public IEnumerable<DB_ParentElementViewModel> VisibleParents
        {
            get
            {
                if (LoadedParentViewModels == null) return null;
                else
                {
                    IEnumerable<DB_ParentElementViewModel> filteredByParentType = ParentTypeFilter == FlexData.ParentType.Unspecified
                        ? LoadedParentViewModels
                        : LoadedParentViewModels.Where(dbWordViewModel => dbWordViewModel.ParentType.Equals((byte)ParentTypeFilter));
                    return filteredByParentType
                        .OrderBy(dbWordViewModel => dbWordViewModel.DefaultRealization);
                }
            }
        }

        private Task LoadVisibleParentsAsync() => Task.Run(() =>
        {
            LoadedParentViewModels.AddRange(ViewModelsFor(FlexData.Context.DB_Parents));
            OnPropertyChanged("VisibleParents");
        });

        private FlexData.ParentType ParentTypeFilter = FlexData.ParentType.Unspecified;

        /// <summary>Filter the VisibleParents to display only those of type <paramref name="filter"/>.</summary>
        internal void SetParentTypeFilter(FlexData.ParentType parentType)
        {
            ParentTypeFilter = parentType;
            OnPropertyChanged("VisibleParents");
        }

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
