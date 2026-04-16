using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UIFramework;

namespace RevitUndocumentedAPI.Common
{
#if REVIT2022
    [StructLayout(LayoutKind.Explicit, Size = 72)]
    public unsafe struct CalculationsPanelWrapper
    {
        [FieldOffset(48)]
        private readonly IntPtr _window;

        [FieldOffset(56)]
        private readonly IntPtr _items;

        [FieldOffset(64)]
        private readonly IntPtr _windowInteropHelper;
#elif REVIT2025 || REVIT2026
    [StructLayout(LayoutKind.Explicit, Size = 88)]
    public unsafe struct CalculationsPanelWrapper
    {
        [FieldOffset(64)]
        private readonly IntPtr _window;

        [FieldOffset(72)]
        private readonly IntPtr _items;

        [FieldOffset(80)]
        private readonly IntPtr _windowInteropHelper;
#endif
        public ObservableCollection<CalculationsPanelItem> Items
        {
            get
            {
                var handle = (GCHandle)_items;
                if (handle.Target != null)
                {
                    return (ObservableCollection<CalculationsPanelItem>)handle.Target;
                }

                return null;
            }
        }
    }
}
