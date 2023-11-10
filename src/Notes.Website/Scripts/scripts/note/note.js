async function init() {
    var editor = await window.CKEDITOR.ClassicEditor.create(
        document.querySelector('#app'), 
        {
            toolbar: [],
            removePlugins: [
                'AIAssistant',
                'Mentions',
                'GeneralHtmlSupport',
                'StyleEditing',
                'Style',
                'HtmlEmbed',
                'Highlight',
                'ExportPdf',
                'ExportWord',
                'CKBox',
                'CKFinder',
                'EasyImage',
                'Base64UploadAdapter',
                'RealTimeCollaborativeComments',
                'RealTimeCollaborativeTrackChanges',
                'RealTimeCollaborativeRevisionHistory',
                'PresenceList',
                'Comments',
                'TrackChanges',
                'TrackChangesData',
                'RevisionHistory',
                'Pagination',
                'WProofreader',
                'MathType',
                'SlashCommand',
                'Template',
                'DocumentOutline',
                'FormatPainter',
                'TableOfContents',
                'PasteFromOfficeEnhanced'
            ]
        }
    );

    editor.keystrokes.set('Ctrl+L', 'removeFormat');
    editor.keystrokes.set('Ctrl+L', 'removeFormat');

    document.getElementById("skip-btn").addEventListener("click", () => editor.editing.view.focus());
}

(async () => init())();