import { makeStyles, Theme } from '@material-ui/core'
import React from 'react'

interface Props {
  title?: string
  text: string
  color: string
}
interface StyleProps {
  color: string
}
const useStyle = makeStyles<Theme, StyleProps>((theme) => ({
  badge: (props) => ({
    display: 'inline-block',
    color: theme.palette.type === 'dark' ? props.color : 'black',
    backgroundColor: theme.palette.type === 'dark' ? '#222' : props.color,
    borderRadius: 5,
    paddingLeft: theme.spacing(1),
    paddingRight: theme.spacing(1),
    marginRight: theme.spacing(1),
    fontSize: '80%',
  }),
  badgeContainer: (props) => ({
    display: 'inline-block',
    padding: 0,
    marginLeft: 0,
    marginRight: theme.spacing(1),
    fontSize: '80%',
  }),
  badgeLeft: (props) => ({
    display: 'inline-block',
    color: theme.palette.type === 'dark' ? props.color : 'black',
    backgroundColor: theme.palette.type === 'dark' ? '#222' : props.color,
    borderTopLeftRadius : 5,
    borderBottomLeftRadius: 5,
    paddingLeft: theme.spacing(1),
    paddingRight: theme.spacing(1),
  }),
  badgeRight: (props) => ({
    display: 'inline-block',
    color: theme.palette.type === 'dark' ? '#f0f0f0' : '#444444',
    backgroundColor: theme.palette.type === 'dark' ? '#444444' : '#f0f0f0',
    borderTopRightRadius : 5,
    borderBottomRightRadius: 5,
    paddingLeft: theme.spacing(1),
    paddingRight: theme.spacing(1),
    marginLeft: 0,
  }),
}))

const Label: React.FC<Props> = ({ title, text, color }) => {
  const classes = useStyle({ color })
  return (
    <>
      {title && (
        <div className={classes.badgeContainer}>
          <div className={classes.badgeLeft}>{title}</div>
          <div className={classes.badgeRight}>{text}</div>
        </div>
      )}
      {!title && <div className={classes.badge}>{text}</div>}
    </>
  )
}

export default Label
