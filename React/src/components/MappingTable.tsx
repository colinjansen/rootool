import { MenuItem, Select, Typography } from '@material-ui/core'
import { DataGrid, GridCellParams, GridColumns, GridRowData } from '@material-ui/data-grid'
import { ColumnTypes, RooToolAreas } from 'data'
import { Dictionary, MappingData } from 'models'
import React from 'react'


interface Props {
  data: MappingData[]
  handleUpdate?: (data: MappingData[]) => void
}
const MappingTable: React.FC<Props> = ({ data, handleUpdate }) => {
  const [rows, setRows] = React.useState<GridRowData[]>([])

  React.useEffect(() => {
    setRows(data as GridRowData[])
  }, [data])

  interface SelectEditProps {
    options: Dictionary<string>
  }

  const SelectEdit: React.FC<GridCellParams & SelectEditProps> = (props) => {
    const { id, value, api, field, options } = props
    const handleChange = (event: any) => {
      let newRows:GridRowData[] = [ ...rows ]
      newRows[id as number] = { ...newRows[id as number], [field]: event.target.value }
      setRows(newRows)
      handleUpdate?.(newRows as MappingData[])
      api.setCellMode(id, field, 'view')
    }
    return (
      <Select fullWidth labelId="map-entry-type-label" id="map-entry-type" value={value} onChange={handleChange}>
        {Object.keys(options).map((key) => (
          <MenuItem key={key} value={key}>
            {options[key]}
          </MenuItem>
        ))}
      </Select>
    )
  }
  
  const renderTypeSelectCell = (params: GridCellParams) => {
    return <SelectEdit options={ColumnTypes} {...params} />
  }
  
  const renderMapToSelectCell = (params: GridCellParams) => {
    return <SelectEdit options={RooToolAreas} {...params} />
  }
  
  const renderTypeCell = (params: GridCellParams) => {
    return ColumnTypes[params.value as string] ?? ''
  }
  
  const renderMapToCell = (params: GridCellParams) => {
    return RooToolAreas[params.value as string] ?? ''
  }
  
  const renderExampleContent = (params: GridCellParams) => {
    var values = params.value as object[]
    return values[0]
  }

  const columns: GridColumns = [
    { field: 'id', headerName: 'Column Offset', width: 50 },
    { field: 'columnName', headerName: 'Column name', flex: 1 },
    { field: 'type', headerName: 'Data Type', editable: true, flex: 1, renderCell: renderTypeCell, renderEditCell: renderTypeSelectCell },
    { field: 'mapTo', headerName: 'Map To Location', editable: true, flex: 1, renderCell: renderMapToCell, renderEditCell: renderMapToSelectCell },
    { field: 'exampleContent', headerName: 'Example Data', editable: false, flex: 1, renderCell: renderExampleContent }
  ]

  if (data.length === 0) {
    return <Typography variant="caption">No mapping data</Typography>
  }
  return (
    <div style={{ display: 'flex', height: '100%' }}>
      <div style={{ flexGrow: 1 }}>
        <DataGrid density="compact" autoHeight pageSize={15} rows={rows} columns={columns} />
      </div>
    </div>
  )
}

export default MappingTable
