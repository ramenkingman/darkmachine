mergeInto(LibraryManager.library, {
    RegisterUnloadEvent: function () {
        window.addEventListener("beforeunload", function (e) {
            if (typeof unityInstance !== 'undefined' && unityInstance) {
                unityInstance.SendMessage('PlayFabManager', 'SaveDataFromJS');
            }
        });
    }
});