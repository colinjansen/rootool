import Drawer from '@material-ui/core/Drawer'
import { verticalHeader } from 'components/style/shared'
import React from 'react'
import { makeStyles } from '@material-ui/core/styles'

const useStyles = makeStyles((theme) => ({
  drawer: (props: any) => ({
    margin: 20,
    marginLeft: 40,
    '@media (min-width: 600px)': {
      minWidth: props.width || 300,
      maxWidth: props.width || 500,
    },
  }),
  paper: {
    '@media (max-width: 600px)': {
      width: '100%',
    },
  },
}))

interface Props {
  open: boolean
  title: string
  width?: number
  handleClose?: () => void
}
const GenericDrawer: React.FC<Props> = ({ open, title, children, width, handleClose }) => {
  const classes = useStyles({ width })
  const vert = verticalHeader(300)
  return (
    <>
      <Drawer classes={{ paper: classes.paper }} anchor="right" open={open} onClose={() => handleClose && handleClose()}>
        <div role="presentation" className={classes.drawer}>
          <div style={vert}>
            {title}
          </div>
          {children}
        </div>
      </Drawer>
    </>
  )
}

export default GenericDrawer
