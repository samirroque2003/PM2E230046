namespace PM2E230046;

public partial class LoginPage : ContentPage
{
	private int _intentosFallidos;

	public LoginPage()
	{
		InitializeComponent();
	}

	private void OnMostrarClaveChanged(object? sender, CheckedChangedEventArgs e)
	{
		ClaveEntry.IsPassword = !e.Value;
	}

	private async void OnLoginClicked(object? sender, EventArgs e)
	{
		ErrorLabel.IsVisible = false;

		if (string.IsNullOrWhiteSpace(CorreoEntry.Text) || string.IsNullOrWhiteSpace(ClaveEntry.Text))
		{
			MostrarError("Escribe tu correo y contraseña.");
			return;
		}

		LoginButton.IsEnabled = false;
		LoadingIndicator.IsVisible = true;
		LoadingIndicator.IsRunning = true;

		try
		{
			await PracticaUI.ApiService.LoginAsync(CorreoEntry.Text.Trim(), ClaveEntry.Text);
			await Shell.Current.GoToAsync("//PerfilPage");
		}
		catch (PracticaUI.ApiException ex)
		{
			if (ex.Codigo == System.Net.HttpStatusCode.Unauthorized)
			{
				_intentosFallidos++;

				if (_intentosFallidos >= 3)
				{
					MostrarError("Has superado los 3 intentos. El botón de login ha sido bloqueado.");
				}
				else
				{
					MostrarError($"{ex.Message} Intento {_intentosFallidos} de 3.");
				}
			}
			else
			{
				MostrarError(ex.Message);
			}
		}
		catch (HttpRequestException)
		{
			MostrarError("No se pudo conectar con la API.");
		}
		finally
		{
			LoginButton.IsEnabled = _intentosFallidos < 3;
			LoadingIndicator.IsVisible = false;
			LoadingIndicator.IsRunning = false;
		}
	}

	private void MostrarError(string mensaje)
	{
		ErrorLabel.Text = mensaje;
		ErrorLabel.IsVisible = true;
	}


}