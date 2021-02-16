using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlexibleRealization;
using Flex.Database;

namespace Flex.UserInterface.ViewModels
{ 
    internal class BoundSynsetsViewModel : INotifyPropertyChanged
    {
        private object elementFilter;
        /// <summary>An element to which synsets can be bound, used to filter synset-to-element bindings.</summary>
        internal object ElementFilter
        {
            get => elementFilter;
            set
            {
                elementFilter = value;
                OnPropertyChanged("VisibleBindings");
            }
        }

        internal void Clear()
        {
            ElementFilter = null;
            ElementsAndBindings.Clear();
            OnPropertyChanged("VisibleBindings");
        }

        /// <summary>The collection of all synset-to-element bindings known to the editor</summary>
        private Dictionary<object, HashSet<SynsetToElementBinding>> ElementsAndBindings = new Dictionary<object, HashSet<SynsetToElementBinding>>();

        /// <summary>The collection of bindings that match ElementFilter.</summary>
        public IEnumerable<SynsetToElementBinding> VisibleBindings
        {
            get
            {
                if (ElementFilter == null) return Enumerable.Empty<SynsetToElementBinding>();
                HashSet<SynsetToElementBinding> listOfBindingsForTheFilteredElement;
                return ElementsAndBindings.TryGetValue(ElementFilter, out listOfBindingsForTheFilteredElement)
                    ? listOfBindingsForTheFilteredElement
                    : Enumerable.Empty<SynsetToElementBinding>();
            }
        }

        private HashSet<SynsetToElementBinding> BindingsForElement(object element)
        {
            HashSet<SynsetToElementBinding> result;
            if (!ElementsAndBindings.TryGetValue(element, out result))
            {
                result = new HashSet<SynsetToElementBinding>();
                ElementsAndBindings.Add(element, result);
            }
            return result;
        }


        /// <summary>Add a new binding between <paramref name="boundElement"/> and the synset with ID <paramref name="boundSynsetID"/>.</summary>
        internal void AddBinding(object elementToBind, int synsetIDToBind)
        {
            HashSet<SynsetToElementBinding> bindingsForTheElement = BindingsForElement(elementToBind);
            SynsetToElementBinding existingBinding = bindingsForTheElement.FirstOrDefault(binding => binding.SynsetID.Equals(synsetIDToBind));
            if (existingBinding == null)
                bindingsForTheElement.Add(new SynsetToElementBinding { SynsetID = synsetIDToBind });
            OnPropertyChanged("VisibleBindings");
        }

        /// <summary>Delete <paramref name="bindingToDelete"/>.=</summary>
        internal void DeleteBinding(SynsetToElementBinding bindingToDelete)
        {
            BindingsForElement(ElementFilter).Remove(bindingToDelete);
            OnPropertyChanged("VisibleBindings");
        }


        internal async void LoadBindingsFor(IElementTreeNode tree)
        {
            IEnumerable<IElementTreeNode> treeElements = tree.WithAllDescendentBuilders;
            foreach (SynsetToElementBinding eachBinding in await FlexData.Context.GetSynsetBindingsForTree(tree.FlexDB_ID))
            {
                IElementTreeNode eachBoundElement = treeElements.Single(element => element.FlexDB_ID.Equals(eachBinding.ElementID));
                BindingsForElement(eachBoundElement).Add(eachBinding);
            }
            OnPropertyChanged("VisibleBindings");
        }

        /// <summary>Save Bindings to the Flex database.</summary>
        internal void SaveBindingsFor(IElementTreeNode tree)
        {
            // Before saving the bindings, make sure each one has a valid ElementID
            foreach (object eachElement in ElementsAndBindings.Keys)
            {
                int eachElementID = IDofElement(eachElement);
                foreach (SynsetToElementBinding eachBinding in ElementsAndBindings[eachElement])
                    eachBinding.ElementID = eachElementID;
            }
            // Aggregate all the bindings into one IEnumerable
            IEnumerable<SynsetToElementBinding> bindingsForTheseElements = ElementsAndBindings.Aggregate(new List<SynsetToElementBinding>(), (allBindings, bindingsForElement) =>
            {
                allBindings.AddRange(bindingsForElement.Value);
                return allBindings;
            });
            FlexData.Context.SaveSynsetToElementBindingsAsync(bindingsForTheseElements);
        }

        private int IDofElement(object element) => element switch
        {
            IElementTreeNode elementNode => elementNode.FlexDB_ID,
            DB_Parent dbParent => dbParent.ID,
            DB_Word dbWord => dbWord.ID,
            _ => throw new InvalidOperationException("Unknown Element type in SynsetToElementBinding")
        };

        #region Standard implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Standard implementation of INotifyPropertyChanged
    }

}
