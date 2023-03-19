export interface Post {
  id: number,
  title: string,
  content: string,
  createdOn: Date,
  image?: string,

  ownerId: string,

  tagIds: number[],
}

export interface PostFormModel {
  title: string,
  content: string,
  image?: File,
  tagIds?: number[],
}
