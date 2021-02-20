mergeInto(
  LibraryManager.library,
  {
    AddClickListenerForFileDialog: function () {
      document.addEventListener('click', function () {

        var fileuploader = document.getElementById('fileuploader');
        if (!fileuploader) {
          fileuploader = document.createElement('input');
          fileuploader.setAttribute('style', 'display:none;');
          fileuploader.setAttribute('type', 'file');
          fileuploader.setAttribute('id', 'fileuploader');
          fileuploader.setAttribute('class', '');
          document.getElementsByTagName('body')[0].appendChild(fileuploader);

          fileuploader.onchange = function (e) {
            var files = e.target.files;
            for (var i = 0, f; f = files[i]; i++) {
              window.alert("Opening file from URL: ".concat(URL.createObjectURL(f)));
              SendMessage('Import Btn', 'FileDialogResult', URL.createObjectURL(f));
            }
          };
        }
        if (fileuploader.getAttribute('class') == 'focused') {
          fileuploader.setAttribute('class', '');
          fileuploader.click();
        }
      });
    }
  }
);

mergeInto(
  LibraryManager.library,
  {
    FocusFileUploader: function () {
      var fileuploader = document.getElementById('fileuploader');
      if (fileuploader) {
          fileuploader.setAttribute('class', 'focused');
      }
    }
  }
);

mergeInto(LibraryManager.library, {
 
 
  PasteHereWindow: function (sometext) {
	var pastedtext= prompt("Please copy+paste the content of a level.txt file here:", "");
	SendMessage("Import Btn", "ImportFromPasted", pastedtext);
  },
 
});
