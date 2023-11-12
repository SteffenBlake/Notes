export async function initAsync(element) {
    var editor = await window.CKEDITOR.ClassicEditor.create(
        element, 
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

    return editor;
}