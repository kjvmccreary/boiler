// ✅ UPDATED: Test should expect "Active" instead of "Pending"
it('should display Active status for newly created users', () => {
  const user = {
    id: '1',
    email: 'test@example.com',
    firstName: 'Test',
    lastName: 'User',
    emailConfirmed: true, // ✅ UPDATED: Now true
    isActive: true,
    roles: ['User']
  };

  render(<UserList users={[user]} />);
  
  expect(screen.getByText('Active')).toBeInTheDocument();
  expect(screen.queryByText('Pending')).not.toBeInTheDocument(); // ✅ UPDATED: Should not show Pending
});
