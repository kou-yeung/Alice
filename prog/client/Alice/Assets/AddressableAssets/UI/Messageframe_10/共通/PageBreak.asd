@loadcell
@loop
;
@macro name=copyone
@copy dx=0 dy=0 sx=%x sy=0 sw=30 sh=35
@wait time=60
@endmacro
;
*start

@copyone x=0
@copyone x=30
@copyone x=60
@copyone x=90
@copyone x=120
@copyone x=90
@copyone x=60
@copyone x=30

@jump target=*start
