import { Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button } from '@material-ui/core'
import { Alert, AlertTitle } from '@material-ui/lab'
import { DataFolder } from 'data'
import React from 'react'
import http from 'data/httpCommon'
import DataFolderListItem from './DataFolderListItem'

interface Props {
  folder?: DataFolder
  message: string
  handleClose: () => void
  handleDataDeleted: () => void
  endpoint: string
}
const DataDeleteDialog: React.FC<Props> = ({ folder, message, handleClose, handleDataDeleted, endpoint }) => {
  const deleteDataFolderFromShare = async (name: string) => {
    await http.delete(`${endpoint}/${btoa(name)}`)
    handleDataDeleted()
  }
  return (
    <Dialog open={folder !== undefined} onClose={() => handleClose()} aria-labelledby="delete-data-folder" aria-describedby="Delete a data folder">
    <DialogTitle id="delete-data-folder">Delete Data</DialogTitle>
    <DialogContent>
      <DialogContentText id="alert-dialog-description">
        <Alert severity="warning">
          <AlertTitle>Warning</AlertTitle>
          {message}
        </Alert>
        {folder && (
          <DataFolderListItem folder={folder} hideActions />
        )}
      </DialogContentText>
    </DialogContent>
    <DialogActions>
      <Button onClick={() => folder && deleteDataFolderFromShare(folder?.name)} color="secondary">
        Delete
      </Button>
      <Button onClick={() => handleClose()} color="primary" autoFocus>
        Keep
      </Button>
    </DialogActions>
  </Dialog>
  )
}

export default DataDeleteDialog