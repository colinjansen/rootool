import React from 'react'

export const verticalHeader = (w = 200, h = 30) => ({
    position: "absolute",
    fontSize: 20,
    fontWeight: 600,
    color: '#bbbbbb',
    transform: 'rotate(90deg)',
    width: w,
    height: h,
    left: (w-h)/-2,
    top: w/2,
  }) as React.CSSProperties;