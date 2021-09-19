import React from 'react'
import { Box, Button, Grid, IconButton, LinearProgress, Typography, withStyles } from '@material-ui/core'
import DeleteIcon from '@material-ui/icons/Delete'
import http from 'data/httpCommon'

const BorderLinearProgress = withStyles((theme) => ({
  root: {
    height: 15,
    borderRadius: 5,
  },
  colorPrimary: {
    backgroundColor: '#EEEEEE',
  },
  bar: {
    borderRadius: 5,
    backgroundColor: '#1a90ff',
  },
}))(LinearProgress)

interface Props {
  handleDataImported: (data: any) => any
  endpoint?: string
}
const FileUpload: React.FC<Props> = ({ handleDataImported, endpoint = '/data/upload' }) => {
  const [progress, setProgress] = React.useState(0)
  const [uploading, setUploading] = React.useState(false)
  const [selectedFile, setSelectedFile] = React.useState<File | undefined>(undefined)
  const [message, setMessage] = React.useState<string | undefined>(undefined)

  const handleSelectFile = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!event.target.files) {
      return
    }
    if (event.target.files.length === 0) {
      return
    }
    setSelectedFile(event.target.files[0])
  }

  const handleRemoveFile = () => {
    setSelectedFile(undefined)
  }

  const handleUploadFile = (file: File) => {
    setUploading(true)
    let formData = new FormData()
    formData.append('file', file)
    http
      .post(endpoint, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
        onUploadProgress: (event) => {
          setProgress(Math.round((100 * event.loaded) / event.total))
        },
      })
      .then((result) => {
        setMessage('file upload complete')
        handleDataImported(result.data)
      })
      .catch((err: Error) => {
        setMessage(err.message)
      })
      .finally(() => {
        setSelectedFile(undefined)
      })
  }

  return (
    <>
      {selectedFile && uploading && (
        <Box className="mb25" display="flex" alignItems="center">
          <Box width="100%" mr={1}>
            <BorderLinearProgress variant="determinate" value={progress} />
          </Box>
          <Box minWidth={35}>
            <Typography variant="body2" color="textSecondary">{`${progress}%`}</Typography>
          </Box>
        </Box>
      )}
      {!selectedFile && !uploading && (
        <label htmlFor="btn-upload">
          <input id="btn-upload" name="btn-upload" style={{ display: 'none' }} type="file" accept=".zip" onChange={(event) => handleSelectFile(event)} />
          <Button className="btn-choose" variant="outlined" component="span">
            Choose Zip File
          </Button>
        </label>
      )}
      {selectedFile && !uploading && (
        <Grid container alignContent="center">
          <Grid item>
            {selectedFile.name}
            <IconButton aria-label="delete" onClick={() => handleRemoveFile()}>
              <DeleteIcon />
            </IconButton>
          </Grid>
          <Grid item>
            <Button className="btn-upload" color="primary" variant="contained" component="span" onClick={() => handleUploadFile(selectedFile)}>
              Upload
            </Button>
          </Grid>
        </Grid>
      )}
      {message && <div>{message}</div>}
    </>
  )
}

export default FileUpload
