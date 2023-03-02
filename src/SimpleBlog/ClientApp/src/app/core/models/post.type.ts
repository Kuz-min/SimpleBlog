export interface Post {
  id: number,
  title: string,
  content: string,
  createdOn: Date,
  image?: string,

  ownerId: string,

  tagIds: number[],
}
