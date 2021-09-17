import { ListItem, ListItemAvatar, Avatar, ListItemText, ListItemSecondaryAction, IconButton } from '@material-ui/core'
import moment from 'moment'
import React from 'react'
import Label from 'components/Label'
import FolderIcon from '@material-ui/icons/Folder'
import DeleteIcon from '@material-ui/icons/Delete'
import { DataFolder } from 'data'

const bytesToHumanReadable = (bytes: number) => {
  const K = 1024
  if (bytes < K) return `${bytes} B`
  const M = K * 1024
  if (bytes < M) return `${(bytes / K).toFixed(2)} KB`
  const G = M * 1024
  if (bytes < G) return `${(bytes / M).toFixed(2)} MB`
  const T = G * 1024
  if (bytes < T) return `${(bytes / T).toFixed(2)} GB`
  return `< 1 TB`
}

interface Props {
  folder: DataFolder
  selected?: boolean
  hideActions?: boolean
  handleSelect?: (folder: DataFolder) => void
  handleDelete?: (folder: DataFolder) => void
}
const DataFolderListItem: React.FC<Props> = ({ folder, handleDelete, handleSelect, selected=false, hideActions = false }) => {
  return (
    <ListItem selected={selected} onClick={() => handleSelect?.(folder)}>
      <ListItemAvatar>
        <Avatar>
          <FolderIcon />
        </Avatar>
      </ListItemAvatar>
      <ListItemText
        primary={folder.name}
        secondary={
          <>
            <Label color="lightblue" title="size" text={bytesToHumanReadable(folder.size)} />
            <Label color="lightgreen" title="created" text={moment(folder.createdOn).fromNow()} />
          </>
        }
      />
      {!hideActions && (
        <ListItemSecondaryAction>
          <IconButton edge="end" aria-label="delete" onClick={() => handleDelete?.(folder)}>
            <DeleteIcon />
          </IconButton>
        </ListItemSecondaryAction>
      )}
    </ListItem>
  )
}

export default DataFolderListItem