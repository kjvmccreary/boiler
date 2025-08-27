import { Box, Typography, Card, CardContent, Alert } from '@mui/material';
import { useParams } from 'react-router-dom';

export function BuilderPage() {
  const { id } = useParams<{ id: string }>();

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Workflow Builder
      </Typography>
      
      <Card>
        <CardContent>
          <Alert severity="info" sx={{ mb: 2 }}>
            Coming Soon! The ReactFlow-based workflow builder will be implemented next.
          </Alert>
          
          <Typography variant="body1">
            This page will feature:
          </Typography>
          
          <ul>
            <li>ReactFlow canvas for drag-and-drop workflow design</li>
            <li>Node palette with Start, End, HumanTask, Automatic, Gateway, Timer nodes</li>
            <li>Property panel for editing node configurations</li>
            <li>Save/Load workflow definitions</li>
            <li>Validation before publishing</li>
          </ul>
          
          {id && (
            <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
              {id === 'new' ? 'Creating new workflow' : `Editing workflow ID: ${id}`}
            </Typography>
          )}
        </CardContent>
      </Card>
    </Box>
  );
}

export default BuilderPage;
