export interface IFileView {
    Content: string,
    Mime: string,
    LanguageAlphaName: string,
    Name: string,
}

export class FileView implements IFileView {
    Content = "";
    Mime = "";
    LanguageAlphaName = "";
    Name= "";
}

export const EmptyFileView = new FileView();