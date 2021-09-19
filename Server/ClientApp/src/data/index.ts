import { Dictionary } from 'models'

export enum RooToolArea {
  unknown = '',
  aesAerialPhotos = 'aes_aerial_photos',
}
export const RooToolAreas: Dictionary<string> = {
  [RooToolArea.unknown]: 'Unknown',
  [RooToolArea.aesAerialPhotos]: 'AES Aerial Photos',
}

export interface DataFolder {
  name: string
  createdOn: Date
  size: number
}


export enum ColumnType {
  unknown = 'unknown',
  string = 'string',
  commaSeparatedString = 'comma_separated_string',
  integer = 'integer',
  decimal = 'decimal',
  date = 'date',
}
export const ColumnTypes: Dictionary<string> = {
  [ColumnType.unknown]: 'Unknown',
  [ColumnType.string]: 'Text data',
  [ColumnType.commaSeparatedString]: 'Comma separated string data',
  [ColumnType.integer]: 'Numeric data (non floating point)',
  [ColumnType.decimal]: 'Numeric data (floating point)',
  [ColumnType.date]: 'Date Expression in standard format'
}
