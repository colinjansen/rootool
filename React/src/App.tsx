import { AppBar, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, IconButton, makeStyles, Toolbar, Tooltip, Typography, Box, TextField, Button } from '@material-ui/core'
import AccountTreeIcon from '@material-ui/icons/AccountTree'
import FolderIcon from '@material-ui/icons/Folder'
import GenericDrawer from 'components/GenericDrawer'
import DataFoldersContainer from 'containers/DataFoldersContainer'
import DataMappingsContainer from 'containers/DataMappingsContainer'
import { ColumnType, DataFolder, RooToolArea } from 'data'
import React from 'react'
import http from 'data/httpCommon'
import moment from 'moment'
import { v4 as uuid } from 'uuid'
import './App.css'
import { DataMap, MappingData } from 'models'
import MappingTable from 'components/MappingTable'
import DataImportDialog from 'components/DataImportDialog'
import DataDeleteDialog from 'components/DataDeleteDialog'

const useStyle = makeStyles((theme) => ({
  grow: {
    flexGrow: 1,
  },
  title: {
    display: 'none',
    [theme.breakpoints.up('sm')]: {
      display: 'block',
    },
  },
}))

const handleSampleDataImport = (result: { data: string[][] }, overwrite: boolean = false) => {
  const newData: MappingData[] = []
  const names = result.data[0]
  const lines = result.data.length
  names.forEach((name, index) => {
    const examples: string[] = []
    for (var i = 1; i < lines; i++) {
      examples.push(result.data[1][index])
    }
    let entry = newData[index]
    if (!entry || (entry && entry.columnName !== name)) {
      newData[index] = {
        id: index,
        type: ColumnType.string,
        columnName: name,
        exampleContent: examples,
        mapTo: RooToolArea.unknown,
      }
    }
  })
  return newData
}

const App: React.FC = () => {
  const [showDataFolders, setShowDataFolders] = React.useState(false)
  const [showMappings, setShowMappings] = React.useState(false)
  const [dataFolder, setDataFolder] = React.useState<DataFolder | undefined>(undefined)
  const [dataMaps, setDataMaps] = React.useState<DataMap[]>([])
  const [dataFolders, setDataFolders] = React.useState<DataFolder[]>([])
  const [currentMap, setCurrentMap] = React.useState<DataMap | undefined>(undefined)
  const [importDataDialogVisible, setImportDataDialogVisible] = React.useState(false)
  const [deleteDataFolder, setDeleteDataFolder] = React.useState<DataFolder | undefined>(undefined)

  const getDataMaps = async () => {
    var result = await http.get(`map/list`)
    if (result.status === 200) {
      const maps = result.data as { id: string; name: string; data: string }[]
      setDataMaps(maps.map((m) => ({ ...m, data: JSON.parse(m.data) })))
    }
  }

  const saveDataMap = async (map: DataMap) => {
    var result = await http.post(`map/upsert`, { id: map.id, name: map.name, data: JSON.stringify(map.data) })
    if (result.status === 200) {
      getDataMaps()
    }
  }

  const deleteDataMap = async (map: DataMap) => {
    var result = await http.delete(`map/delete/${map.id}`)
    if (result.status === 200) {
      getDataMaps()
    }
  }

  const loadDataFoldersFromShare = async () => {
    const result = await http.get<DataFolder[]>('/data/list')
    setDataFolders(result.data)
  }

  const importSampleData = async (folder: DataFolder) => {
    var result = await http.post(`map/buildFromData/${btoa(folder.name)}`)
    var data = handleSampleDataImport(result.data)
    setCurrentMap({
      id: uuid(),
      name: `Generated Map ${moment().toISOString()}`,
      data,
    })
  }

  React.useEffect(() => {
    loadDataFoldersFromShare()
    getDataMaps()
  }, [])

  const classes = useStyle()
  return (
    <>
      <div className={classes.grow}>
        <AppBar position="static">
          <Toolbar>
            <Typography className={classes.title} variant="h6" noWrap>
              Data Conversion Utilities
            </Typography>
            <div className={classes.grow} />
            <Tooltip title="Data Folders">
              <IconButton aria-label="data folders" color="inherit" onClick={() => setShowDataFolders(true)}>
                <FolderIcon />
              </IconButton>
            </Tooltip>
            <IconButton aria-label="data mappings" color="inherit" onClick={() => setShowMappings(true)}>
              <Tooltip title="Mappings">
                <AccountTreeIcon />
              </Tooltip>
            </IconButton>
          </Toolbar>
        </AppBar>
      </div>

      <GenericDrawer
        title="Data Folders"
        handleClose={() => {
          setShowDataFolders(false)
          setDataFolder(undefined)
        }}
        open={showDataFolders}
      >
        <Typography variant="h4">Data Folders</Typography>
        <DataFoldersContainer dataFolders={dataFolders} handleSelect={(folder) => setDataFolder(folder)} handleDelete={(folder) => setDeleteDataFolder(folder)} />
        <Button disabled={dataFolder === undefined} onClick={() => dataFolder && importSampleData(dataFolder)}>
          Generate Structure from Data
        </Button>
        <Button onClick={() => setImportDataDialogVisible(true)}>Upload Data</Button>
      </GenericDrawer>

      <GenericDrawer title="Data Mappings" handleClose={() => setShowMappings(false)} open={showMappings}>
        <Typography variant="h4">Data Mappings</Typography>
        <DataMappingsContainer dataMaps={dataMaps} handleEdit={(map) => setCurrentMap(map)} />
      </GenericDrawer>

      {currentMap && (
        <Dialog fullWidth maxWidth="lg" open={currentMap !== undefined} onClose={() => setCurrentMap(undefined)} aria-labelledby="max-width-dialog-title">
          <DialogTitle id="max-width-dialog-title">Edit Mapping Data</DialogTitle>
          <DialogContent>
            <DialogContentText>Edit the data to map to the desired destinations.</DialogContentText>
            <Box style={{ marginBottom: 10 }}>
              <TextField fullWidth label="Map Name" size="medium" name="map-name" value={currentMap.name} onChange={(e) => setCurrentMap({ ...currentMap, name: e.target.value })} />
            </Box>
            <MappingTable handleUpdate={(data) => setCurrentMap({ ...currentMap, data })} data={currentMap.data} />
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setCurrentMap(undefined)} color="secondary">
              Close
            </Button>
            <Button onClick={() => {
              deleteDataMap(currentMap)
              setCurrentMap(undefined)
            }} color="secondary">
              Delete
            </Button>
            <Button
              onClick={() => {
                saveDataMap(currentMap)
                setCurrentMap(undefined)
              }}
              color="primary"
            >
              Save Data Mapping Configuration
            </Button>
          </DialogActions>
        </Dialog>
      )}

      <DataImportDialog
        message={`Upload must be a zip file. The root of the zip file should contain the data in a CSV (comma separated values) file. Any photos referenced in the CSV file should be in a folder called 'photos'`}
        endpoint="/data/upload"
        handleDataImported={(data) => {
          loadDataFoldersFromShare()
          setImportDataDialogVisible(false)
        }}
        open={importDataDialogVisible}
        handleClose={() => setImportDataDialogVisible(false)}
      />

      <DataDeleteDialog
        message="This will permanently delete this data folder. Are you sure this is what you want?"
        endpoint="/data/delete"
        handleDataDeleted={() => {
          loadDataFoldersFromShare()
          setDeleteDataFolder(undefined)
        }}
        folder={deleteDataFolder}
        handleClose={() => setDeleteDataFolder(undefined)}
      />
    </>
  )
}

export default App
