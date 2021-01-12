using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Flex.Database.UserInterface.ViewModels
{
    public class ParentBuildersViewModel : INotifyPropertyChanged
    {
        internal ParentBuildersViewModel()
        {
            LoadVisibleParentsAsync();
        }

        private Task LoadVisibleParentsAsync() => Task.Run(() =>
        {
            VisibleParents = FlexData.Context.LoadAllDB_ParentElements()
                .Select(dbParent => new ParentViewModel
                {
                    DefaultRealization = dbParent.ParentDefaultRealization,
                    Model = FlexData.Context.LoadParent(dbParent.ID)
                })
                .ToList();
            OnPropertyChanged("VisibleParents");
        });

        public List<ParentViewModel> VisibleParents { get; set; }

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }
}
