import retrofit2.http.POST
import retrofit2.http.Body
import com.mindraco.moodtab.data.LoginPayload
import com.mindraco.moodtab.data.LoginResponseDto
import retrofit2.Response

interface ApiService {
    @POST("autenticacionlogin/login")
    suspend fun login(@Body payload: LoginPayload): Response<LoginResponseDto>
}