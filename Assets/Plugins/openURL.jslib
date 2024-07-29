mergeInto(LibraryManager.library, {
  openURL: function (url) {
    var jsString = Pointer_stringify(url);
    openURL(jsString);
  },
  copyDisplayName: function (name) {
    var jsString = Pointer_stringify(name);
    navigator.clipboard.writeText(jsString).then(function() {
      console.log('Display name copied to clipboard: ' + jsString);
    }).catch(function(err) {
      console.error('Could not copy display name to clipboard: ', err);
    });
  }
});

