mergeInto(LibraryManager.library, {
  openURL: function (url) {
    var jsString = Pointer_stringify(url);
    openURL(jsString);
  }
});
