import { Avatar, IconButton, List, ListItem, ListItemAvatar, ListItemSecondaryAction, ListItemText } from '@material-ui/core'
import EditIcon from '@material-ui/icons/Edit'
import FolderIcon from '@material-ui/icons/Folder'
import Label from 'components/Label'
import { DataMap } from 'models'
import React from 'react'


interface Props {
  dataMaps: DataMap[]
  handleEdit?: (map: DataMap) => void
}
const DataMappingsContainer: React.FC<Props> = ({ dataMaps, handleEdit }) => {
  return (
    <>
      <div>
        <List dense>
          {dataMaps.map((map: DataMap) => (
            <ListItem>
              <ListItemAvatar>
                <Avatar>
                  <FolderIcon />
                </Avatar>
              </ListItemAvatar>
              <ListItemText
                primary={map.name}
                secondary={
                  <>
                    <Label color="lightblue" title="size" text={map.id} />
                  </>
                }
              />
                <ListItemSecondaryAction>
                  <IconButton edge="end" aria-label="edit" onClick={() => handleEdit?.(map)}>
                    <EditIcon />
                  </IconButton>
                </ListItemSecondaryAction>
            </ListItem>
          ))}
        </List>
      </div>
    </>
  )
}

export default DataMappingsContainer
