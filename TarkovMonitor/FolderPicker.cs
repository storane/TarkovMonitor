using Microsoft.WindowsAPICodePack.Dialogs;

namespace TarkovMonitor
{
    public interface IFolderPicker
    {
        public string DisplayFolderPicker();
    }

    public class FolderPicker : IFolderPicker
    {
            public string DisplayFolderPicker()
            {
                var dialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true
                };
                CommonFileDialogResult result = dialog.ShowDialog();
                    if (result == CommonFileDialogResult.Ok && dialog.FileName != null)
                        return dialog.FileName;
                    return "";
            }
    }
}
