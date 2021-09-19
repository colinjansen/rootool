import { Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button } from '@material-ui/core'
import React from 'react'
import FileUpload from './FileUpload'

interface Props {
  open: boolean
  message: string
  handleClose?: () => void
  handleDataImported?: (data: any) => void
  endpoint: string
}
const DataImportDialog: React.FC<Props> = ({ open, message, handleClose, handleDataImported, endpoint }) => {
  return (
    <Dialog open={open} onClose={() => handleClose?.()} aria-labelledby="form-dialog-title">
      <DialogTitle id="form-dialog-title">Data Import</DialogTitle>
      <DialogContent>
        <DialogContentText>{message}</DialogContentText>
        <FileUpload endpoint={endpoint} handleDataImported={(data) => handleDataImported?.(data)} />
      </DialogContent>
      <DialogActions>
        <Button onClick={() => handleClose?.()} color="secondary">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default DataImportDialog
