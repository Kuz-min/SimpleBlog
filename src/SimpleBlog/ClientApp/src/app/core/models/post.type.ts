export interface Post {
  id: number,
  title: string,
  content: string,
  createdOn: Date,

  ownerId: number,

  tagIds: number[],
}
