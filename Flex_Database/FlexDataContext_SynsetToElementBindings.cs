using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flex.Database
{
    public partial class FlexDataContext
    {
        public Task<IEnumerable<SynsetToElementBinding>> GetSynsetBindingsForTree(int rootID) => Task.Run(() =>
        {
            IEnumerable<GetNodesIDsForTreeResult> nodeIDsInTheTree = GetNodesIDsForTree(rootID);
            return SynsetToElementBindings
                .Where(binding => nodeIDsInTheTree.Any(node => node.ID.Equals(binding.ElementID)))
                .AsEnumerable();
        });

        public Task SaveSynsetToElementBindingsAsync(IEnumerable<SynsetToElementBinding> bindingsToSave) => Task.Run(() => SaveSynsetToElementBindings(bindingsToSave));

        private void SaveSynsetToElementBindings(IEnumerable<SynsetToElementBinding> bindingsToSave)
        {
            IEnumerable<SynsetToElementBinding> bindingsToUpdate = bindingsToSave.Intersect(SynsetToElementBindings);
            foreach (SynsetToElementBinding eachExistingBinding in bindingsToUpdate)
                UpdateSynsetToElementBinding(eachExistingBinding);
            IEnumerable<SynsetToElementBinding> bindingsToInsert = bindingsToSave.Except(SynsetToElementBindings);
            SynsetToElementBindings.InsertAllOnSubmit(bindingsToInsert);
            SubmitChanges();
        }

    }
}
