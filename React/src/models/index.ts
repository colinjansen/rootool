export interface Dictionary<T> {
  [key: string] : T
}

export interface MappingData {
  id: number, 
  columnName: string, 
  exampleContent: any, 
  type: string, 
  mapTo: string
}

export interface DataMap {
  id: string
  name: string
  data: MappingData[]
}
