namespace PM2E230046;

public partial class PerfilPage : ContentPage
{
	public PerfilPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await CargarPerfilAsync();
	}

	private async Task CargarPerfilAsync()
	{
		ErrorLabel.IsVisible = false;
		LoadingIndicator.IsVisible = true;
		LoadingIndicator.IsRunning = true;

		try
		{
			var perfil = await PracticaUI.ApiService.ObtenerPerfilAsync();

			NombreLabel.Text = perfil.Nombre;
			InicialesLabel.Text = ObtenerIniciales(perfil.Nombre);
			DisponibleSwitch.IsToggled = perfil.Disponible;
			ExperienciaSlider.Value = Math.Clamp(perfil.AniosExperiencia, 0, 15);
			ActualizarNivel(ExperienciaSlider.Value);
		}
		catch (PracticaUI.ApiException ex)
		{
			MostrarError(ex.Message);
		}
		catch (HttpRequestException)
		{
			MostrarError("No se pudo conectar con la API.");
		}
		finally
		{
			LoadingIndicator.IsVisible = false;
			LoadingIndicator.IsRunning = false;
		}
	}

	private void OnExperienciaChanged(object? sender, ValueChangedEventArgs e)
	{
		ActualizarNivel(e.NewValue);
	}

	private void ActualizarNivel(double experiencia)
	{
		var anios = (int)Math.Round(experiencia);
		ExperienciaLabel.Text = $"{anios} años";

		if (anios <= 2)
		{
			NivelLabel.Text = "Nivel inicial";
			NivelBorder.BackgroundColor = Color.FromArgb("#DCFCE7");
			ExperienciaSlider.MinimumTrackColor = Color.FromArgb("#DCFCE7");
		}
		else if (anios <= 7)
		{
			NivelLabel.Text = "Nivel intermedio";
			NivelBorder.BackgroundColor = Color.FromArgb("#E0F2FE");
			ExperienciaSlider.MinimumTrackColor = Color.FromArgb("#E0F2FE");
		}
		else
		{
			NivelLabel.Text = "Nivel Avanzado";
			NivelBorder.BackgroundColor = Color.FromArgb("#FEF3C7");
			ExperienciaSlider.MinimumTrackColor = Color.FromArgb("#FEF3C7");
		}
	}

	private async void OnCerrarSesionClicked(object? sender, EventArgs e)
	{
		PracticaUI.ApiService.CerrarSesion();
		await Shell.Current.GoToAsync("//LoginPage");
	}

	private void MostrarError(string mensaje)
	{
		ErrorLabel.Text = mensaje;
		ErrorLabel.IsVisible = true;
	}

	private static string ObtenerIniciales(string nombre)
	{
		var partes = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		return partes.Length switch
		{
			0 => "?",
			1 => partes[0][..Math.Min(2, partes[0].Length)].ToUpperInvariant(),
			_ => $"{partes[0][0]}{partes[^1][0]}".ToUpperInvariant()
		};
	}
}