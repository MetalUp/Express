export interface IFileView {
    Content: string,
    Mime: string,
    LanguageAlphaName: string
}

export class FileView implements IFileView {
    Content = "";
    Mime = "";
    LanguageAlphaName = "";
}

export const EmptyFileView = new FileView();