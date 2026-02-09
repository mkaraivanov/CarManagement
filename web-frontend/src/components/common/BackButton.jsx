import { Button } from '@mui/material';
import { ArrowBack } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

const BackButton = ({
  to = -1,
  label = 'Back',
  disabled = false,
  variant = 'outlined',
  fullWidth = false,
  icon = <ArrowBack />,
  onClick = null,
  ...props
}) => {
  const navigate = useNavigate();

  const handleClick = () => {
    if (onClick) {
      onClick();
    } else if (typeof to === 'number') {
      navigate(to);
    } else {
      navigate(to);
    }
  };

  return (
    <Button
      variant={variant}
      startIcon={icon}
      onClick={handleClick}
      disabled={disabled}
      fullWidth={fullWidth}
      {...props}
    >
      {label}
    </Button>
  );
};

export default BackButton;
