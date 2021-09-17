import { List, Typography } from '@material-ui/core'
import DataFolderListItem from 'components/DataFolderListItem'
import { DataFolder } from 'data'
import React from 'react'

interface Props {
  dataFolders: DataFolder[]
  handleSelect?: (folder: DataFolder | undefined) => void
  handleDelete?: (folder: DataFolder) => void
}
const DataFoldersContainer: React.FC<Props> = ({ dataFolders, handleSelect, handleDelete }) => {
  const [selectedDataFolder, setSelectedDataFolder] = React.useState<DataFolder | undefined>(undefined)

  return (
    <>
      {dataFolders.length === 0 && <Typography variant="body1">No Data Folders</Typography>}
      {dataFolders.length > 0 && (
        <List dense>
          {dataFolders.map((folder) => (
            <DataFolderListItem
              selected={folder === selectedDataFolder}
              folder={folder}
              handleSelect={(folder) => {
                if (folder === selectedDataFolder) {
                  setSelectedDataFolder(undefined)
                  handleSelect?.(undefined)
                } else {
                  setSelectedDataFolder(folder)
                  handleSelect?.(folder)
                }
              }}
              handleDelete={(folder) => handleDelete?.(folder)}
            />
          ))}
        </List>
      )}
    </>
  )
}

export default DataFoldersContainer
